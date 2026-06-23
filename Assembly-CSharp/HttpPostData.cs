using System;
using Server.Data;

internal class HttpPostData
{
	public HttpPostData()
	{
		this._ClientVerifyGift = null;
		this.LifeTime = 0f;
		this.HaveSend = false;
	}

	public ClientVerifyGift _ClientVerifyGift;

	public float LifeTime;

	public bool HaveSend;

	public string HttpPost = string.Empty;
}
