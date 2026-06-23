using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class UnZipFloClass
{
	public string unZipFile(string TargetFile, string fileDir)
	{
		string text = " ";
		string result;
		try
		{
			ZipInputStream zipInputStream = new ZipInputStream(File.OpenRead(TargetFile.Trim()));
			ZipEntry nextEntry;
			while ((nextEntry = zipInputStream.GetNextEntry()) != null)
			{
				string name = nextEntry.Name;
				string text2 = "\\";
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				string text3 = name.Replace(text2, directorySeparatorChar.ToString());
				string text4 = Path.GetDirectoryName(text3);
				string text5 = text4;
				string text6 = "\\";
				char directorySeparatorChar2 = Path.DirectorySeparatorChar;
				text4 = text5.Replace(text6, directorySeparatorChar2.ToString());
				string fileName = Path.GetFileName(text3);
				char directorySeparatorChar3 = Path.DirectorySeparatorChar;
				string text7 = fileDir + directorySeparatorChar3.ToString() + text4;
				char directorySeparatorChar4 = Path.DirectorySeparatorChar;
				if (!Directory.Exists(fileDir + directorySeparatorChar4.ToString() + text4))
				{
					Directory.CreateDirectory(text7);
				}
				if (fileName != string.Empty)
				{
					string text8 = text7;
					char directorySeparatorChar5 = Path.DirectorySeparatorChar;
					FileStream fileStream = File.Create(text8 + directorySeparatorChar5.ToString() + fileName);
					byte[] array = new byte[2048];
					for (;;)
					{
						int num = zipInputStream.Read(array, 0, array.Length);
						if (num <= 0)
						{
							break;
						}
						fileStream.Write(array, 0, num);
					}
					fileStream.Close();
				}
			}
			zipInputStream.Close();
			result = text;
		}
		catch (Exception ex)
		{
			result = "1; " + ex.Message;
		}
		return result;
	}
}
