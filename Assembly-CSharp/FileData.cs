using System;

public class FileData
{
	public FileData(string fileName, long fileSize, int type = -1)
	{
		this.FileName = fileName;
		this.FileSize = fileSize;
		this.Type = type;
	}

	public string FileName;

	public long FileSize;

	public int Type = -1;
}
