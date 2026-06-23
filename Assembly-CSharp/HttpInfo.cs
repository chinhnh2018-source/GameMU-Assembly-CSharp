using System;
using System.Collections.Generic;

public class HttpInfo
{
	public string _url;

	public string _type = "GET";

	public float _timeOut = 10f;

	public Dictionary<string, string> _formData;

	public byte[] _byteData;

	public SimpleHttpTask.HttpCallback callbackDel;
}
