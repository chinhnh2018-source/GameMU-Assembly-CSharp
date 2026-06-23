using System;

public class DownLoadFile
{
	public DownLoadFile(string _path, byte[] _fileBytes, int type = -1)
	{
		this.path = _path;
		this.fileBytes = _fileBytes;
		this.type = type;
	}

	public string path;

	public byte[] fileBytes;

	public int type;
}
