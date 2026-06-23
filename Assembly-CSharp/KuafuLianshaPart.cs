using System;
using System.Collections;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class KuafuLianshaPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		base.StartCoroutine<bool>(this.TickProc());
	}

	private void ShowInfo(object result)
	{
		if (result == null)
		{
			return;
		}
		if (result is HuanYingSiYuanLianSha)
		{
			HuanYingSiYuanLianSha huanYingSiYuanLianSha = result as HuanYingSiYuanLianSha;
			this.killedIcon.gameObject.SetActive(false);
			this.killedName.gameObject.SetActive(false);
			this.killed.gameObject.SetActive(false);
			if (huanYingSiYuanLianSha.Side == 2)
			{
				this.killer.spriteName = "killer";
			}
			else
			{
				this.killer.spriteName = "killed";
			}
			this.killerName.text = Global.FormatRoleNameZoneid(huanYingSiYuanLianSha.ZoneID, huanYingSiYuanLianSha.Name, 0, 0);
			this.killerIcon.ImageURL = string.Format("NetImages/Face/{0}0_0.png", Global.CalcOriginalOccupationID(huanYingSiYuanLianSha.Occupation).ToString());
			int num = huanYingSiYuanLianSha.LianShaType - 1;
			if (num > 3)
			{
				num = 3;
			}
			this.lianshaType.spriteName = string.Format("type_{0}", num);
			this.lianshaType.transform.localPosition = new Vector3(25f, 8f, -1f);
			this.lianshaType.MakePixelPerfect();
		}
		if (result is HuanYingSiYuanLianshaOver)
		{
			HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = result as HuanYingSiYuanLianshaOver;
			this.killedIcon.gameObject.SetActive(true);
			this.killedName.gameObject.SetActive(true);
			this.killed.gameObject.SetActive(true);
			if (huanYingSiYuanLianshaOver.KillerSide == 2)
			{
				this.killer.spriteName = "killer";
			}
			else
			{
				this.killer.spriteName = "killed";
			}
			if (huanYingSiYuanLianshaOver.KilledSide == 2)
			{
				this.killed.spriteName = "killer";
			}
			else
			{
				this.killed.spriteName = "killed";
			}
			this.killerName.text = Global.FormatRoleNameZoneid(huanYingSiYuanLianshaOver.KillerZoneID, huanYingSiYuanLianshaOver.KillerName, 0, 0);
			this.killerIcon.ImageURL = string.Format("NetImages/Face/{0}0_0.png", Global.CalcOriginalOccupationID(huanYingSiYuanLianshaOver.KillerOccupation).ToString());
			this.killedName.text = Global.FormatRoleNameZoneid(huanYingSiYuanLianshaOver.KilledZoneID, huanYingSiYuanLianshaOver.KilledName, 0, 0);
			this.killedIcon.ImageURL = string.Format("NetImages/Face/{0}0_0.png", Global.CalcOriginalOccupationID(huanYingSiYuanLianshaOver.KilledOccupation).ToString());
			this.lianshaType.spriteName = string.Format("type_{0}", "zhongjie");
			this.lianshaType.transform.localPosition = new Vector3(72f, 8f, -1f);
			this.lianshaType.transform.localScale = new Vector3(218f, 26f, 0f);
			this.lianshaType.MakePixelPerfect();
		}
		if (result is HuanYingSiYuanAddScore)
		{
			HuanYingSiYuanAddScore huanYingSiYuanAddScore = result as HuanYingSiYuanAddScore;
			this.killedIcon.gameObject.SetActive(false);
			this.killedName.gameObject.SetActive(false);
			this.killed.gameObject.SetActive(false);
			if (huanYingSiYuanAddScore.Side == 2)
			{
				this.killer.spriteName = "killer";
			}
			else
			{
				this.killer.spriteName = "killed";
			}
			this.killerName.text = Global.FormatRoleNameZoneid(huanYingSiYuanAddScore.ZoneID, huanYingSiYuanAddScore.Name, 0, 0);
			this.killerIcon.ImageURL = string.Format("NetImages/Face/{0}0_0.png", Global.CalcOriginalOccupationID(huanYingSiYuanAddScore.Occupation).ToString());
			this.lianshaType.spriteName = string.Format("type_{0}", "success");
			this.lianshaType.MakePixelPerfect();
			this.lianshaType.transform.localPosition = new Vector3(60f, 8f, -1f);
		}
	}

	private IEnumerator TickProc()
	{
		for (;;)
		{
			if (Global.listLianShaObject.Count > 0)
			{
				object result = Global.listLianShaObject[0];
				this.ShowInfo(result);
				Global.listLianShaObject.Remove(result);
				MUDebug.Log<string>(new string[]
				{
					"**********" + Global.listLianShaObject.Count + "**********"
				});
			}
			else
			{
				PlayZone.GlobalPlayZone.CloseKuafuLianshaWindow();
			}
			yield return new WaitForSeconds(5f);
		}
		yield break;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage killerIcon;

	public ShowNetImage killedIcon;

	public TextBlock killerName;

	public TextBlock killedName;

	public UISprite lianshaType;

	public UISprite killer;

	public UISprite killed;
}
