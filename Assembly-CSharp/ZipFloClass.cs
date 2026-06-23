using System;
using System.IO;
using HSGameEngine.GameEngine.Logic;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

public class ZipFloClass
{
	public void ZipFileLimitSize(string strFile, string strZip)
	{
		this.strZip = strZip;
		if (strFile.get_Chars(strFile.Length - 1) != Path.DirectorySeparatorChar)
		{
			strFile += Path.DirectorySeparatorChar;
		}
		this.zipLimitSize(strFile, strFile);
		this.s.Finish();
		this.s.Close();
	}

	private void zipLimitSize(string strFile, string staticFile)
	{
		if (this.s == null)
		{
			this.s = new ZipOutputStream(File.Create(string.Concat(new object[]
			{
				this.strZip,
				"_",
				this.count,
				".zip"
			})));
			this.s.SetLevel(6);
		}
		if (strFile.get_Chars(strFile.Length - 1) != Path.DirectorySeparatorChar)
		{
			strFile += Path.DirectorySeparatorChar;
		}
		Crc32 crc = new Crc32();
		foreach (string text in Directory.GetFileSystemEntries(strFile))
		{
			string extension = Path.GetExtension(text);
			if (!(extension == ".meta"))
			{
				string fileName = Path.GetFileName(text);
				if (!(fileName == ".svn"))
				{
					if (Directory.Exists(text))
					{
						this.zipLimitSize(text, staticFile);
					}
					else
					{
						FileStream fileStream = File.OpenRead(text);
						byte[] array = new byte[fileStream.Length];
						fileStream.Read(array, 0, array.Length);
						this.totalFileSize += (long)array.Length;
						string text2 = text.Substring(staticFile.LastIndexOf("\\") + 1);
						ZipEntry zipEntry = new ZipEntry(text2);
						zipEntry.DateTime = DateTime.Now;
						zipEntry.Size = fileStream.Length;
						fileStream.Close();
						crc.Reset();
						crc.Update(array);
						zipEntry.Crc = crc.Value;
						this.s.PutNextEntry(zipEntry);
						this.s.Write(array, 0, array.Length);
						if (this.totalFileSize > (long)(((!Context.IsHaiwai) ? 5 : 10) * 1024 * 1024))
						{
							this.s.Finish();
							this.s.Close();
							this.totalFileSize = 0L;
							this.count++;
							this.s = new ZipOutputStream(File.Create(string.Concat(new object[]
							{
								this.strZip,
								"_",
								this.count,
								".zip"
							})));
							this.s.SetLevel(6);
						}
					}
				}
			}
		}
	}

	public void ZipFile(string strFile, string strZip)
	{
		if (strFile.get_Chars(strFile.Length - 1) != Path.DirectorySeparatorChar)
		{
			strFile += Path.DirectorySeparatorChar;
		}
		ZipOutputStream zipOutputStream = new ZipOutputStream(File.Create(strZip));
		zipOutputStream.SetLevel(6);
		this.zip(strFile, zipOutputStream, strFile);
		zipOutputStream.Finish();
		zipOutputStream.Close();
	}

	private void zip(string strFile, ZipOutputStream s, string staticFile)
	{
		if (strFile.get_Chars(strFile.Length - 1) != Path.DirectorySeparatorChar)
		{
			strFile += Path.DirectorySeparatorChar;
		}
		Crc32 crc = new Crc32();
		foreach (string text in Directory.GetFileSystemEntries(strFile))
		{
			if (Directory.Exists(text))
			{
				this.zip(text, s, staticFile);
			}
			else
			{
				FileStream fileStream = File.OpenRead(text);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				string text2 = text.Substring(staticFile.LastIndexOf("\\") + 1);
				ZipEntry zipEntry = new ZipEntry(text2);
				zipEntry.DateTime = DateTime.Now;
				zipEntry.Size = fileStream.Length;
				fileStream.Close();
				crc.Reset();
				crc.Update(array);
				zipEntry.Crc = crc.Value;
				s.PutNextEntry(zipEntry);
				s.Write(array, 0, array.Length);
			}
		}
	}

	private long totalFileSize;

	private int count;

	private string strZip;

	private ZipOutputStream s;
}
