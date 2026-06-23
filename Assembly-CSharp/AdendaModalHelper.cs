using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AdendaModalHelper : MonoBehaviour
{
	public ResourceResLoader ShowMedalModalByID(int modelID, float scale = 1f)
	{
		if (null != this.medalModal)
		{
			Object.Destroy(this.medalModal.gameObject);
			this.medalModal = null;
		}
		this.medalModal = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(base.gameObject, this.medalModal.gameObject, false);
		Transform transform = this.medalModal.transform;
		transform.localPosition = new Vector3(0f, 0f, -0.8f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(this.medalModal.transform);
		return UIHelper.LoadModelResource(this.medalModal, modelID, scale, null);
	}

	public DPSelectedItemEventHandler slideEventHandler;

	public int modelID;

	private Modal3DShow medalModal;

	private Vector2 delta = Vector2.zero;
}
