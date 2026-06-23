using System;
using System.IO;
using Server.Tools;

namespace Server.Protocol
{
	internal class TCPPolicy
	{
		public static void LoadPolicyServerFile(string file)
		{
			TCPPolicy.PolicyServerFileContent = File.ReadAllBytes(file);
			byte[] array = new byte[TCPPolicy.PolicyServerFileContent.Length + 1];
			DataHelper.CopyBytes(array, 0, TCPPolicy.PolicyServerFileContent, 0, TCPPolicy.PolicyServerFileContent.Length);
			array[array.Length - 1] = 0;
			TCPPolicy.PolicyServerFileContent = array;
		}

		public const string POLICY_STRING = "<policy-file-request/>";

		public static byte[] PolicyServerFileContent;
	}
}
